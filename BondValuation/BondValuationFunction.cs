using BondValuation.Core.Models;
using BondValuation.Core.Servises;
using BondValuation.Core.Utils.Mappings;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace BondValuation;

public class BondValuationFunction
{
    private readonly ILogger<BondValuationFunction> _logger;
    private readonly IBondValuationService _valuationService;

    public BondValuationFunction(ILogger<BondValuationFunction> logger, IBondValuationService bondValuationService)
    {
        _logger = logger;
        _valuationService = bondValuationService;
    }

    [Function(nameof(BondValuationFunction))]
    [BlobOutput("bonds/output/bonds_valued_{name}", Connection = "BondValuationStorageConnection")]
    public Task<string> Run(
    [BlobTrigger("bonds/input/{name}", Connection = "BondValuationStorageConnection")] string inputFileContent)
    {
        _logger.LogInformation($"Processing blob: {inputFileContent} at {DateTime.UtcNow}");

        CsvConfiguration config = new(CultureInfo.InvariantCulture)
        {
            Delimiter = ";", // Use semicolon delimiter for European format
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            BadDataFound = null
        };

        using CsvReader csvReader = new(new StringReader(inputFileContent), config);
        csvReader.Context.RegisterClassMap<BondRecordMap>();

        List<BondRecord> records = csvReader.GetRecords<BondRecord>().ToList();
        _logger.LogInformation($"Read {records.Count} bond records from {inputFileContent}");

        List<ValuationResult> valuationResults = [];
        try
        {
            valuationResults = [.. records.Select(_valuationService.CalculateBondValuation)];
        }
        catch (Exception e)
        {
            _logger.LogError($"Error during bond valuation: {e.Message}");
        }


        // Write results to CSV string
        using StringWriter writer = new();
        CsvConfiguration outputConfig = new(CultureInfo.InvariantCulture)
        {
            Delimiter = ";" // Use semicolon delimiter for output
        };

        using CsvWriter csvWriter = new(writer, outputConfig);
        csvWriter.WriteRecords(valuationResults);

        var output = writer.ToString();

        _logger.LogInformation($"Processed {valuationResults.Count} records successfully.");

        return Task.FromResult(output);
    }
}
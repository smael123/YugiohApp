# Yugioh CardApp

Work in progress. Base for a future C# web application that allows you to search cards from the Yugioh Trading Card Game and filter based on multiple values.

Card information is pulled from an API from **[YGOPRODECK](https://db.ygoprodeck.com/api-guide/)**. Currently only the repository class library and an integration test project is available.

To run the test project you will need a JSON file containing all of the cards. To do that, save the JSON file returned from the Get all cards endpoint listed on the YGOPRODECK page linked above.

Running the unit test project
1. Download a JSON file with all the cards from the **[YGOPRODECK](https://db.ygoprodeck.com/api-guide/)** website using the 'Get all cards' endpoint.
2. On the YugiohCardRepositoryTests.cs file set the value of the JSON_FILE_PATH constant to the file path where you saved your JSON file.
3. Run the integration tests to confirm everything is OK. Unfortunately some of my intergration tests are based on things that could change with the addition of new cards.

This project is not affiliated with KONAMI or YGOPRODECK.
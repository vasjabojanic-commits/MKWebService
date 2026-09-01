using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

public class Customer
{
    public string id { get; set; }
    public string? name { get; set; }
    public string? address { get; set; }
    public string? headquarters { get; set; }
    public string? country { get; set; }
    public string? telephone_1 { get; set; }
    public string? email_1 { get; set; }
    public string? cre_date { get; set; }
    public string? cha_date { get; set; }

    public Customer()
    {
        id = "";
    }

    public Customer(string ParId)
    {
        id = ParId;
    }

    public Boolean FillFromJsonElement(JsonElement el)
    {
        if(el.TryGetProperty("id", out JsonElement idElement))
        {
            id = idElement.GetString();
            if(el.TryGetProperty("name", out JsonElement nameElement))
                name = nameElement.GetString();
            if(el.TryGetProperty("address", out JsonElement addressElement))
                address = addressElement.GetString();
            if(el.TryGetProperty("headquarters", out JsonElement headquartersElement))
                headquarters = headquartersElement.GetString();
            if(el.TryGetProperty("country", out JsonElement countryElement))
                country = countryElement.GetString();
            if(el.TryGetProperty("telephone_1", out JsonElement telephone_1Element))
                telephone_1 = telephone_1Element.GetString();
            if(el.TryGetProperty("email_1", out JsonElement email_1Element))
                email_1 = email_1Element.GetString();
            if(el.TryGetProperty("cre_date", out JsonElement cre_dateElement))
                cre_date = cre_dateElement.GetString();
            if(el.TryGetProperty("cha_date", out JsonElement cha_dateElement))
                cha_date = cha_dateElement.GetString();
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public async Task<(string Result, string Id)> SendToDb(string ConnString)
    {
        await using var connection =
            new SqlConnection(ConnString);

        await using var command =
            new SqlCommand("dbo.update_Customers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

        command.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = (object?)id ?? DBNull.Value;

        command.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = (object?)name ?? DBNull.Value;

        command.Parameters.Add("@address", SqlDbType.NVarChar, 500).Value = (object?)address ?? DBNull.Value;

        command.Parameters.Add("@headquarters", SqlDbType.NVarChar, 200).Value = (object?)headquarters ?? DBNull.Value;

        command.Parameters.Add("@country", SqlDbType.NVarChar, 100).Value = (object?)country ?? DBNull.Value;

        command.Parameters.Add("@telephone_1", SqlDbType.NVarChar, 50).Value = (object?)telephone_1 ?? DBNull.Value;

        command.Parameters.Add("@email_1", SqlDbType.NVarChar, 200).Value = (object?)email_1 ?? DBNull.Value;

        command.Parameters.Add("@cre_date", SqlDbType.NVarChar, 50).Value = (object?)cre_date ?? DBNull.Value;

        command.Parameters.Add("@cha_date", SqlDbType.NVarChar, 50).Value = (object?)cha_date ?? DBNull.Value;

        await connection.OpenAsync();

        int RowsAffected =
            await command.ExecuteNonQueryAsync();

        return (
            "Success",
            "0"
        );
    }

    
}
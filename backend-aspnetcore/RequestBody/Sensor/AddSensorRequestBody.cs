namespace BackendAspNetCore.RequestBody.Sensor;

public class AddSensorRequestBody
{
    public string MacAddress { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
}
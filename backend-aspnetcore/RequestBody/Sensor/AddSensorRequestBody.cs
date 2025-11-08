namespace BackendAspNetCore.RequestBody.Sensor;

public class AddSensorRequestBody
{
    public string MacAddress { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
}
public class OpenRouteServiceResponse
{
    public List<Feature> features { get; set; } = new();
}

public class Feature
{
    public GeometryData geometry { get; set; } = new();
    public FeatureProperties properties { get; set; } = new();
}

public class GeometryData
{
    public string type { get; set; } = "";
    public List<double[]> coordinates { get; set; } = new();
}

public class FeatureProperties
{
    public Summary summary { get; set; } = new();
}

public class Summary
{
    public double distance { get; set; }
    public double duration { get; set; }
}
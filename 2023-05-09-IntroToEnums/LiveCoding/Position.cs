public interface IpAddr {};

public record struct V4(int p0, int p1, int p2, int p3) : IpAddr; 
public record struct V6(string Value) : IpAddr;
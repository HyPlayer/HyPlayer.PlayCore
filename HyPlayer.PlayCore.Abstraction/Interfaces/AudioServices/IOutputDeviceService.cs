namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

/// <summary>
/// 可以更改输出设备
/// </summary>
public interface IOutputDeviceChangeableService
{
    /// <summary>
    /// 获取可输出设备列表
    /// </summary>
    /// <returns></returns>
    public Task<List<OutputDeviceBase>> GetOutputDevices();

    /// <summary>
    /// 设置输出设备
    /// </summary>
    /// <param name="device">设备信息</param>
    /// <returns></returns>
    public Task SetOutputDevices(OutputDeviceBase device);
}

public abstract class OutputDeviceBase
{
    public required string Name { get; set; }
}
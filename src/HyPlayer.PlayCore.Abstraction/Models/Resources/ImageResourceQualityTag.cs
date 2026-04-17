namespace HyPlayer.PlayCore.Abstraction.Models.Resources;

public class ImageResourceQualityTag : ResourceQualityTag
{
    public ImageResourceQualityTag(int pixelX, int pixelY)
    {
        PixelX = pixelX;
        PixelY = pixelY;
    }
    public int PixelX { get; }
    public int PixelY { get; }
    
    public override string ToString()
    {
        return $"{PixelX}y{PixelY}";
    }
}
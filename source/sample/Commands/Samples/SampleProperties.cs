using Ciat.CiatCommand;
using Microsoft.Extensions.Logging;

namespace sample.Commands.Samples;

public class SampleProperties : ICiatCommand
{
  public byte ByteProperty { get; set; }
  public sbyte SByteProperty { get; set; }
  public short? ShortProperty { get; set; }
  public ushort? UShortProperty { get; set; }
  public int? IntProperty { get; set; }
  public uint? UIntProperty { get; set; }
  public long? LongProperty { get; set; }
  public ulong? ULongProperty { get; set; }
  public float? FloatProperty { get; set; }
  public double? DoubleProperty { get; set; }
  public decimal? DecimalProperty { get; set; }
  public required bool BoolProperty { get; set; }
  public required char CharProperty { get; set; }
  public string? StringProperty { get; set; }

  public void Execute(ILogger logger)
  {
    new List<string>
    {
      $"Executed class name: '{nameof(SampleProperties)}'",
      "properties:",
      $"{nameof(ByteProperty)}={ByteProperty} ({ByteProperty.GetType()})",
      $"{nameof(SByteProperty)}={SByteProperty} ({SByteProperty.GetType()})",
      $"{nameof(ShortProperty)}={ShortProperty} ({ShortProperty?.GetType()})",
      $"{nameof(UShortProperty)}={UShortProperty} ({UShortProperty?.GetType()})",
      $"{nameof(IntProperty)}={IntProperty} ({IntProperty?.GetType()})",
      $"{nameof(UIntProperty)}={UIntProperty} ({UIntProperty?.GetType()})",
      $"{nameof(LongProperty)}={LongProperty} ({LongProperty?.GetType()})",
      $"{nameof(ULongProperty)}={ULongProperty} ({ULongProperty?.GetType()})",
      $"{nameof(FloatProperty)}={FloatProperty} ({FloatProperty?.GetType()})",
      $"{nameof(DoubleProperty)}={DoubleProperty} ({DoubleProperty?.GetType()})",
      $"{nameof(DecimalProperty)}={DecimalProperty} ({DecimalProperty?.GetType()})",
      $"{nameof(BoolProperty)}={BoolProperty} ({BoolProperty.GetType()})",
      $"{nameof(CharProperty)}={CharProperty} ({CharProperty.GetType()})",
      $"{nameof(StringProperty)}={StringProperty} ({StringProperty?.GetType()})"
    }
    .ForEach(line => logger.LogInformation(line));
  }
}

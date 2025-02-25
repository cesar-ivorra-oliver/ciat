using ciat.ciatCommand;

namespace sample.Commands.Samples;

public class Sample : IciatCommand
{
  public byte ByteValue { get; set; }
  public sbyte SByteValue { get; set; }
  public short ShortValue { get; set; }
  public ushort UShortValue { get; set; }
  public int IntValue { get; set; }
  public uint UIntValue { get; set; }
  public long LongValue { get; set; }
  public ulong ULongValue { get; set; }
  public float FloatValue { get; set; }
  public double DoubleValue { get; set; }
  public decimal DecimalValue { get; set; }
  public bool BoolValue { get; set; }
  public char CharValue { get; set; }
  public string StringValue { get; set; }

  public void Execute()
  {
    Console.WriteLine(string.Join("\n", new[]
    {
      $"Executed class name: '{nameof(Sample)}'",
      "properties:",
      $"{nameof(ByteValue)}={ByteValue} ({ByteValue.GetType()})",
      $"{nameof(SByteValue)}={SByteValue} ({SByteValue.GetType()})",
      $"{nameof(ShortValue)}={ShortValue} ({ShortValue.GetType()})",
      $"{nameof(UShortValue)}={UShortValue} ({UShortValue.GetType()})",
      $"{nameof(IntValue)}={IntValue} ({IntValue.GetType()})",
      $"{nameof(UIntValue)}={UIntValue} ({UIntValue.GetType()})",
      $"{nameof(LongValue)}={LongValue} ({LongValue.GetType()})",
      $"{nameof(ULongValue)}={ULongValue} ({ULongValue.GetType()})",
      $"{nameof(FloatValue)}={FloatValue} ({FloatValue.GetType()})",
      $"{nameof(DoubleValue)}={DoubleValue} ({DoubleValue.GetType()})",
      $"{nameof(DecimalValue)}={DecimalValue} ({DecimalValue.GetType()})",
      $"{nameof(BoolValue)}={BoolValue} ({BoolValue.GetType()})",
      $"{nameof(CharValue)}={CharValue} ({CharValue.GetType()})",
      $"{nameof(StringValue)}={StringValue} ({StringValue.GetType()})"
    }));
  }
}

# ciat
Cesar Ivorra Automation Tool

## Comand to execute a command
```sh
  ciatLauncher.exe <comand_name> --<command_property> <command_value> ...
```


### Execution of Sample command

```sh
ciatLauncher.exe --ByteValue 255 --SByteValue -128 --ShortValue -32768 --UShortValue 65535 --IntValue 2147483647 --UIntValue 4294967295 --LongValue -9223372036854775808 --ULongValue 18446744073709551615 --FloatValue 3.14 --DoubleValue 2.718 --DecimalValue 1.618 --BoolValue true --CharValue A
```

### Example result

```sh
Executed class name: DataTypeDemo;
params:
ByteValue=255 (System.Byte),
SByteValue=-128 (System.SByte),
ShortValue=-32768 (System.Int16),
UShortValue=65535 (System.UInt16),
IntValue=2147483647 (System.Int32),
UIntValue=4294967295 (System.UInt32),
LongValue=-9223372036854775808 (System.Int64),
ULongValue=18446744073709551615 (System.UInt64),
FloatValue=3.14 (System.Single),
DoubleValue=2.718 (System.Double),
DecimalValue=1.618 (System.Decimal),
BoolValue=True (System.Boolean),
CharValue=A (System.Char).
```

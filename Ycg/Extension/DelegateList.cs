using System;
using System.Collections.Generic;

namespace Ycg.Extension
{
    public delegate void SetChar(char value);

    public delegate void SetString(string value);

    public delegate void SetInt(int value);

    public delegate void SetUInt(uint value);

    public delegate void SetLong(long value);

    public delegate void SetULong(ulong value);

    public delegate void SetByte(byte value);

    public delegate void SetSByte(sbyte value);

    public delegate void SetBytes(byte[] value);

    public delegate void SetDouble(double value);

    public delegate void SetFloat(float value);

    public delegate void SetBool(bool value);

    public delegate void SetShort(short value);

    public delegate void SetUShort(ushort value);

    public delegate void SetDateTime(DateTime value);

    //-------------------------------------------------------------------

    public delegate void SetChar<T>(T t, char value);

    public delegate void SetString<T>(T t, string value);

    public delegate void SetInt<T>(T t, int value);

    public delegate void SetUInt<T>(T t, uint value);

    public delegate void SetLong<T>(T t, long value);

    public delegate void SetULong<T>(T t, ulong value);

    public delegate void SetByte<T>(T t, byte value);

    public delegate void SetSByte<T>(T t, sbyte value);

    public delegate void SetBytes<T>(T t, byte[] value);

    public delegate void SetDouble<T>(T t, double value);

    public delegate void SetFloat<T>(T t, float value);

    public delegate void SetBool<T>(T t, bool value);

    public delegate void SetShort<T>(T t, short value);

    public delegate void SetUShort<T>(T t, ushort value);

    public delegate void SetDateTime<T>(T t, DateTime value);
}
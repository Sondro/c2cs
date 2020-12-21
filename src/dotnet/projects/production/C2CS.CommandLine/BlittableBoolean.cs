// Copyright (c) Lucas Girouard-Stranks (https://github.com/lithiumtoast). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;

/// <summary>
///     A boolean value type with the same memory layout as a <see cref="byte" /> in both managed and unmanaged
///     code; equivalent to a standard bool (_Bool) found in C/C++ where <c>0</c> is <c>false</c> and <c>1</c> is
///     <c>true</c>.
/// </summary>
/// <remarks>
///     <para>
///         In .NET, data is often represented in memory differently in managed and unmanaged contexts. Blittable types
///         are data types in software applications which have the same bit representation in both managed and unmanaged
///         contexts. This same bit representation allows the types to be "blitted" (block bit transfer) between
///         contexts with minimal to zero overhead. Understanding the difference between blittable and non-blittable
///         types can aid in having correct and highly performant interoperability for .NET applications with unmanaged
///         code.
///     </para>
/// </remarks>
public readonly struct BlittableBoolean
{
    private readonly byte _value;

    private BlittableBoolean(bool value)
    {
        _value = Convert.ToByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="bool" /> to a <see cref="BlittableBoolean" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="BlittableBoolean" />.</returns>
    public static implicit operator BlittableBoolean(bool value)
    {
        return new(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="BlittableBoolean" /> to a <see cref="bool" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="bool" />.</returns>
    public static implicit operator bool(BlittableBoolean value)
    {
        return Convert.ToBoolean(value._value);
    }

    private static unsafe delegate* unmanaged[Cdecl] <int, int> _myFunc = new IntPtr(5);

    /// <inheritdoc />
    public override string ToString()
    {
        unsafe
        {
            _myFunc = new IntPtr(5);
            var x = _myFunc(_value);
            return Convert.ToBoolean(_value).ToString();
        }
    }
}

using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public class Message {
		protected NetBuffer _message;

		internal Message(NetBuffer message) {
			_message = message;
		}

		public int LengthBits {
			get { return _message.LengthBits; }
			set { _message.LengthBits = value; }
		}
		public int LengthBytes {
			get { return _message.LengthBytes; }
			set { _message.LengthBytes = value; }
		}

		public long Position {
			get { return _message.Position; }
			set { _message.Position = value; }
		}
		public int PositionInBytes {
			get { return _message.PositionInBytes; }
		}

		public bool PeekBoolean() {
			return _message.PeekBoolean();
		}
		public byte PeekByte() {
			return _message.PeekByte();
		}
		public byte[] PeekBytes(int numberOfBytes) {
			return _message.PeekBytes(numberOfBytes);
		}
		public double PeekDouble() {
			return _message.PeekDouble();
		}
		public float PeekFloat() {
			return _message.PeekFloat();
		}
		public int PeekInt32() {
			return _message.PeekInt32();
		}
		public long PeekInt64() {
			return _message.PeekInt64();
		}
		public short PeekInt16() {
			return _message.PeekInt16();
		}
		public sbyte PeekSByte() {
			return _message.PeekSByte();
		}
		public ushort PeekUInt16() {
			return _message.PeekUInt16();
		}
		public uint PeekUInt32() {
			return _message.PeekUInt32();
		}
		public ulong PeekUInt64() {
			return _message.PeekUInt64();
		}
		public string PeekString() {
			return _message.PeekString();
		}

		public bool ReadBoolean() {
			return _message.ReadBoolean();
		}
		public byte ReadByte() {
			return _message.ReadByte();
		}
		public byte[] ReadBytes(int numberOfBytes) {
			return _message.ReadBytes(numberOfBytes);
		}
		public double ReadDouble() {
			return _message.ReadDouble();
		}
		public float ReadFloat() {
			return _message.ReadFloat();
		}
		public int ReadInt32() {
			return _message.ReadInt32();
		}
		public long ReadInt64() {
			return _message.ReadInt64();
		}
		public short ReadInt16() {
			return _message.ReadInt16();
		}
		public sbyte ReadSByte() {
			return _message.ReadSByte();
		}
		public ushort ReadUInt16() {
			return _message.ReadUInt16();
		}
		public uint ReadUInt32() {
			return _message.ReadUInt32();
		}
		public ulong ReadUInt64() {
			return _message.ReadUInt64();
		}
		public string ReadString() {
			return _message.ReadString();
		}

		public void Write(bool value) {
			_message.Write(value);
		}
		public void Write(byte value) {
			_message.Write(value);
		}
		public void Write(byte[] value) {
			_message.Write(value);
		}
		public void Write(double value) {
			_message.Write(value);
		}
		public void Write(float value) {
			_message.Write(value);
		}
		public void Write(int value) {
			_message.Write(value);
		}
		public void Write(long value) {
			_message.Write(value);
		}
		public void Write(short value) {
			_message.Write(value);
		}
		public void Write(sbyte value) {
			_message.Write(value);
		}
		public void Write(uint value) {
			_message.Write(value);
		}
		public void Write(ushort value) {
			_message.Write(value);
		}
		public void Write(ulong value) {
			_message.Write(value);
		}
		public void Write(string value) {
			_message.Write(value);
		}
	}
}

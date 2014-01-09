/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Text;
using FluentCassandra.Thrift.Protocol;

namespace FluentCassandra.Apache.Cassandra
{

  /// <summary>
  /// Represents input splits used by hadoop ColumnFamilyRecordReaders
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CfSplit : TBase
  {

    public string Start_token { get; set; }

    public string End_token { get; set; }

    public long Row_count { get; set; }

    public CfSplit() {
    }

    public CfSplit(string start_token, string end_token, long row_count) : this() {
      this.Start_token = start_token;
      this.End_token = end_token;
      this.Row_count = row_count;
    }

    public void Read (TProtocol iprot)
    {
      bool isset_start_token = false;
      bool isset_end_token = false;
      bool isset_row_count = false;
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.String) {
              Start_token = iprot.ReadString();
              isset_start_token = true;
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.String) {
              End_token = iprot.ReadString();
              isset_end_token = true;
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I64) {
              Row_count = iprot.ReadI64();
              isset_row_count = true;
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
      if (!isset_start_token)
        throw new TProtocolException(TProtocolException.INVALID_DATA);
      if (!isset_end_token)
        throw new TProtocolException(TProtocolException.INVALID_DATA);
      if (!isset_row_count)
        throw new TProtocolException(TProtocolException.INVALID_DATA);
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("CfSplit");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      field.Name = "start_token";
      field.Type = TType.String;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(Start_token);
      oprot.WriteFieldEnd();
      field.Name = "end_token";
      field.Type = TType.String;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(End_token);
      oprot.WriteFieldEnd();
      field.Name = "row_count";
      field.Type = TType.I64;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      oprot.WriteI64(Row_count);
      oprot.WriteFieldEnd();
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CfSplit(");
      sb.Append("Start_token: ");
      sb.Append(Start_token);
      sb.Append(",End_token: ");
      sb.Append(End_token);
      sb.Append(",Row_count: ");
      sb.Append(Row_count);
      sb.Append(")");
      return sb.ToString();
    }

  }

}

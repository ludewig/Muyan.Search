using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Lucene.Net.Documents;

namespace Muyan.Search
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute:Attribute
    {
        public IndexAttribute()
        {
            IsStore = Field.Store.YES;
            FieldType = FieldDataType.Text;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 是否存储
        /// </summary>
        public Field.Store IsStore { get; set; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public FieldDataType FieldType { get; set; }

    }


}

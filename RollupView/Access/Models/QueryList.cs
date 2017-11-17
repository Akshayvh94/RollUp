using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Access.Models
{
    public class QueryList
    {
        public class CreatedBy
        {
            public string id { get; set; }
            public string displayName { get; set; }
        }

        public class LastModifiedBy
        {
            public string id { get; set; }
            public string displayName { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Html
        {
            public string href { get; set; }
        }

        public class Parent
        {
            public string href { get; set; }
        }

        public class Wiql
        {
            public string href { get; set; }
        }

        public class Links
        {
            public Self self { get; set; }
            public Html html { get; set; }
            public Parent parent { get; set; }
            public Wiql wiql { get; set; }
        }

        public class Child
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public DateTime createdDate { get; set; }
            public DateTime lastModifiedDate { get; set; }
            public bool isPublic { get; set; }
            public Links _links { get; set; }
            public string url { get; set; }
            public bool? isFolder { get; set; }
            public bool? hasChildren { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public CreatedBy createdBy { get; set; }
            public DateTime createdDate { get; set; }
            public LastModifiedBy lastModifiedBy { get; set; }
            public DateTime lastModifiedDate { get; set; }
            public bool isFolder { get; set; }
            public bool hasChildren { get; set; }
            public IList<Child> children { get; set; }
            public bool isPublic { get; set; }
            public string url { get; set; }
        }

        public class Query
        {
            public int count { get; set; }
            public IList<Value> value { get; set; }
        }


    }

    public class QueryResponse
    {
        public class CreatedBy
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class LastModifiedBy
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Column
        {
            public string referenceName { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }

        public class Field
        {
            public string referenceName { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }

        public class SortColumn
        {
            public Field field { get; set; }
            public bool descending { get; set; }
        }

        public class Operator
        {
            public string referenceName { get; set; }
            public string name { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Html
        {
            public string href { get; set; }
        }

        public class Parent
        {
            public string href { get; set; }
        }

        public class Wiql
        {
            public string href { get; set; }
        }

        public class Links
        {
            public Self self { get; set; }
            public Html html { get; set; }
            public Parent parent { get; set; }
            public Wiql wiql { get; set; }
        }

        public class Response
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public CreatedBy createdBy { get; set; }
            public DateTime createdDate { get; set; }
            public LastModifiedBy lastModifiedBy { get; set; }
            public DateTime lastModifiedDate { get; set; }
            public string queryType { get; set; }
            public IList<Column> columns { get; set; }
            public IList<SortColumn> sortColumns { get; set; }
            public string wiql { get; set; }
            public bool isPublic { get; set; }
            public string filterOptions { get; set; }
            public Links _links { get; set; }
            public string url { get; set; }
        }
    }
}
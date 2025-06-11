using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The templates.
    /// </summary>
    internal class Templates
    {
        private const string markdownTemplate = @"[
  {
    ""ObjectType"": 0,
    ""Body"": ""# Table: \u0060~ObjectFullName~\u0060\r\n\r\n\u003E ~Description~\r\n\r\n**Columns:**\r\n~Columns~\r\n\r\n**Indexes:**\r\n~Indexes~\r\n\r\n**Constraints:**\r\n~Constraints~\r\n---\r\n"",
    ""Columns"": {
      ""Body"": ""| Ord | Name | Data Type | Description |\r\n|--------|------|-----------|-------------|\r\n~ColumnItem~"",
      ""ColumnRow"": ""| ~ColumnOrd~ | \u0060~ColumnName~\u0060 | ~ColumnDataType~ | ~ColumnDescription~ |""
    },
    ""Constraints"": {
      ""Body"": ""| Constraint Name | Type | Column |\r\n|------------------|------|-------------|\r\n~ConstraintItem~"",
      ""ConstraintRow"": ""| \u0060~ConstraintName~\u0060 | ~ConstraintType~ | ~ConstraintColumn~ |""
    },
    ""Indexes"": {
      ""Body"": ""| Index Name | Type | Columns | Unique |\r\n|------------|------|---------|--------|\r\n~IndexItem~"",
      ""IndexRow"": ""| \u0060~IndexName~\u0060 | ~IndexType~ | ~IndexColumns~ | ~UniqueIndex~ |""
    }
  },
  {
    ""ObjectType"": 1,
    ""Body"": ""# View: \u0060~ObjectFullName~\u0060\r\n\r\n\u003E ~Description~\r\n\r\n**Columns:**\r\n~Columns~\r\n\r\n**Indexes:**\r\n~Indexes~\r\n\r\n**SQL Definition:**\r\n\u0060\u0060\u0060sql\r\n~Definition~\r\n\u0060\u0060\u0060"",
    ""Columns"": {
      ""Body"": ""| Ord | Name | Data Type | Description |\r\n|--------|------|-----------|-------------|\r\n~ColumnItem~"",
      ""ColumnRow"": ""| ~ColumnOrd~ | \u0060~ColumnName~\u0060 | ~ColumnDataType~ | ~ColumnDescription~ |""
    },
    ""Indexes"": {
      ""Body"": ""| Index Name | Type | Columns | Unique |\r\n|------------|------|---------|--------|\r\n~IndexItem~"",
      ""IndexRow"": ""| \u0060~IndexName~\u0060 | ~IndexType~ | ~IndexColumns~ | ~UniqueIndex~ |""
    }
  },
  {
    ""ObjectType"": 2,
    ""Body"": ""# Function: \u0060~ObjectFullName~\u0060\r\n\r\n\u003E ~Description~\r\n\r\n## Parameters\r\n~Parameters~\r\n\r\n## SQL Code\r\n\u0060\u0060\u0060sql\r\n~Definition~\r\n\u0060\u0060\u0060\r\n"",
    ""Parameters"": {
      ""Body"": ""| Ord | Name | Data Type | Direction | Description |\r\n|-----|------|-----------|-----------|-------------|\r\n~ParameterItem~"",
      ""ParameterRow"": ""| ~ParameterOrd~ | \u0060~ParameterName~\u0060 | ~ParameterDataType~ | ~ParameterDirection~ | ~ParameterDescription~ |""
    }
  },
  {
    ""ObjectType"": 3,
    ""Body"": ""# Stored Procedure: \u0060~ObjectFullName~\u0060\r\n\r\n\u003E ~Description~\r\n\r\n## Parameters\r\n~Parameters~\r\n\r\n## SQL Code\r\n\u0060\u0060\u0060sql\r\n~Definition~\r\n\u0060\u0060\u0060\r\n"",
    ""Parameters"": {
      ""Body"": ""| Ord | Name | Data Type | Direction | Description |\r\n|-----|------|-----------|-----------|-------------|\r\n~ParameterItem~"",
      ""ParameterRow"": ""| ~ParameterOrd~ | \u0060~ParameterName~\u0060 | ~ParameterDataType~ | ~ParameterDirection~ | ~ParameterDescription~ |""
    }
  },
  {
    ""ObjectType"": 4,
    ""Body"": ""# Trigger: \u0060~ObjectFullName~\u0060\r\n\r\n\u003E ~Description~\r\n\r\n## Parameters\r\n~Parameters~\r\n\r\n## Trigger SQL Code\r\n\u0060\u0060\u0060sql\r\n~Definition~\r\n\u0060\u0060\u0060\r\n"",
    ""Parameters"": {
      ""Body"": """",
      ""ParameterRow"": """"
    }
  },
  {
    ""ObjectType"": 5,
    ""Body"": ""| Schema | Name | Description |\r\n|--------|------|-------------|\r\n~ObjectItem~"",
    ""ObjectLists"": {
      ""Body"": """",
      ""ObjectRow"": ""| ~ObjectSchema~ | \u0060~ObjectName~\u0060 | ~Description~ |""
    }
  },
  {
    ""ObjectType"": 6,
    ""Body"": ""| ~Header~ |\r\n| ~Align~ |\r\n~Rows~"",
    ""DataTable"": {
      ""Body"": """",
      ""DataRow"": ""| ~Row~ |"",
      ""HeaderCell"": ""~HeaderCell~"",
      ""Cell"": ""~Cell~""
    }
  },
  {
    ""ObjectType"": 7,
    ""Body"": ""# Synonym: \u0060~ObjectFullName~\u0060\r\n\r\n\u003E ~Description~\r\n\r\n### Base Object Name: ~BaseObjectName~\r\n### Base Object Type: ~BaseObjectType~\r\n""
  }
]";

        private const string sharePointTemplate = @"[
  {
    ""ObjectType"": 0,
    ""Body"": ""\u003Ch1\u003ETABLE NAME: ~ObjectFullName~\u003C/h1\u003E\r\n\u003Cp\u003E~Description~\u003C/p\u003E\r\n\u003Cdiv\u003E\r\n~Columns~\r\n\u003C/div\u003E\r\n\u003Cdiv\u003E\r\n    \u003Ch2\u003EIndexes:\u003C/h2\u003E\r\n~Indexes~\r\n\u003C/div\u003E\r\n\u003Cdiv\u003E\r\n    \u003Ch2\u003EConstraints:\u003C/h2\u003E\r\n~Constraints~\r\n\u003C/div\u003E\r\n\u003Chr/\u003E\r\n\u003Cdiv\u003EBack to [[Home]]\u003C/div\u003E"",
    ""Columns"": {
      ""Body"": ""\u003Cdiv\u003E\r\n\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EOrd\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EName\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EData Type\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003ENullable\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EDescription\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~ColumnItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n\u003C/div\u003E\r\n"",
      ""ColumnRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnOrd~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnName~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnDataType~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnNullable~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnDescription~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    },
    ""Constraints"": {
      ""Body"": ""\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EConstraint Name\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EType\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EColumn(s)\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~ConstraintItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n"",
      ""ConstraintRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ConstraintName~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ConstraintType~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ConstraintColumn~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    },
    ""Indexes"": {
      ""Body"": ""\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EIndex Name\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EType\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EColumns\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EUnique\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~IndexItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n"",
      ""IndexRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~IndexName~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~IndexType~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~IndexColumns~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~UniqueIndex~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    }
  },
  {
    ""ObjectType"": 1,
    ""Body"": ""\u003Ch1\u003EVIEW NAME: ~ObjectFullName~\u003C/h1\u003E\r\n\u003Cp\u003E~Description~\u003C/p\u003E\r\n\u003Cdiv\u003E\r\n~Columns~\r\n\u003C/div\u003E\r\n\u003Cdiv\u003E\r\n    \u003Ch2\u003EIndexes:\u003C/h2\u003E\r\n~Indexes~\r\n\u003C/div\u003E\r\n\u003Cdiv\u003E\r\n    \u003Ch2\u003ECode to build the view\u003C/h2\u003E\r\n\u003Cpre style=\u0022white-space: pre-wrap;\u0022\u003E\r\n~Definition~\r\n\u003C/pre\u003E\r\n\u003C/div\u003E\r\n\u003Chr/\u003E\r\n\u003Cdiv\u003EBack to [[Home]]\u003C/div\u003E"",
    ""Columns"": {
      ""Body"": ""\u003Cdiv\u003E\r\n\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EOrd\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EName\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EData Type\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003ENullable\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EDescription\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~ColumnItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n\u003C/div\u003E\r\n"",
      ""ColumnRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnOrd~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnName~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnDataType~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnNullable~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ColumnDescription~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    },
    ""Indexes"": {
      ""Body"": ""\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EIndex Name\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EType\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EColumns\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EUnique\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~IndexItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n"",
      ""IndexRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~IndexName~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~IndexType~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~IndexColumns~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~UniqueIndex~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    }
  },
  {
    ""ObjectType"": 2,
    ""Body"": ""\u003Ch1\u003EFUNCTION NAME: ~ObjectFullName~\u003C/h1\u003E\r\n\u003Cp\u003E~Description~\u003C/p\u003E\r\n\u003Cdiv\u003E\r\n\u003Ch2\u003EParameters:\u003C/h2\u003E\r\n~Parameters~\r\n\u003C/div\u003E\r\n\u003Cdiv\u003E\r\n\u003Ch2\u003ESource code\u003C/h2\u003E\r\n\u003Cpre style=\u0022white-space: pre-wrap;\u0022\u003E~Definition~\r\n\u003C/pre\u003E\r\n\u003C/div\u003E\r\n\u003Chr/\u003E\r\n\u003Cdiv\u003EBack to [[Home]]\u003C/div\u003E \r\n"",
    ""Parameters"": {
      ""Body"": ""\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EOrd\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EName\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EData Type\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EDirection\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EDescription\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~ParameterItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n"",
      ""ParameterRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterOrd~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterName~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterDataType~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterDirection~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterDescription~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    }
  },
  {
    ""ObjectType"": 3,
    ""Body"": ""\u003Ch1\u003EPROCEDURE NAME: ~ObjectFullName~\u003C/h1\u003E\r\n\u003Cp\u003E~Description~\u003C/p\u003E\r\n\u003Cdiv\u003E\r\n\u003Ch2\u003EParameters:\u003C/h2\u003E\r\n~Parameters~\r\n\u003C/div\u003E\r\n\u003Cdiv\u003E\r\n\u003Ch2\u003ESource code\u003C/h2\u003E\r\n\u003Cpre style=\u0022white-space: pre-wrap;\u0022\u003E~Definition~\r\n\u003C/pre\u003E\r\n\u003C/div\u003E\r\n\u003Chr/\u003E\r\n\u003Cdiv\u003EBack to [[Home]]\u003C/div\u003E \r\n"",
    ""Parameters"": {
      ""Body"": ""\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EOrd\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EName\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EData Type\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EDirection\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EDescription\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~ParameterItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n"",
      ""ParameterRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterOrd~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterName~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterDataType~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterDirection~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ParameterDescription~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    }
  },
  {
    ""ObjectType"": 4,
    ""Body"": ""\u003Cpre style=\u0022white-space: pre-wrap;\u0022\u003E~Definition~\r\n\u003C/pre\u003E""
  },
  {
    ""ObjectType"": 5,
    ""Body"": ""\u003Ctable class=\u0022wikitable\u0022 style=\u0022margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\u0022\u003E\r\n\u003Ctbody\u003E\r\n    \u003Ctr\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003ESchema\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EName\u003C/th\u003E\r\n        \u003Cth style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\u0022\u003EDescription\u003C/th\u003E\r\n    \u003C/tr\u003E\r\n~ObjectItem~\r\n\u003C/tbody\u003E\r\n\u003C/table\u003E\r\n"",
    ""ObjectLists"": {
      ""Body"": """",
      ""ObjectRow"": ""    \u003Ctr\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~ObjectSchema~\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E[[~ObjectFullName~|~ObjectName~]]\u003C/td\u003E\r\n        \u003Ctd style=\u0022padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\u0022\u003E~Description~\u003C/td\u003E\r\n    \u003C/tr\u003E\r\n""
    }
  },
  {
    ""ObjectType"": 6,
    ""Body"": ""\u003Ctable class=\u0027wikitable\u0027 style=\u0027border: 1px solid #ccc; font-size: 14px;\u0027\u003E\r\n\u003Cthead\u003E\u003Ctr\u003E~Header~\u003C/tr\u003E\u003C/thead\u003E\r\n\u003Ctbody\u003E~Rows~\u003C/tbody\u003E\r\n\u003C/table\u003E"",
    ""DataTable"": {
      ""Body"": """",
      ""DataRow"": ""\u003Ctr\u003E\r\n~Row~\r\n\u003C/tr\u003E"",
      ""HeaderCell"": ""\u003Cth style=\u0027border: 1px solid #ccc; padding: 4px;\u0027\u003E~HeaderCell~\u003C/th\u003E"",
      ""Cell"": ""\u003Ctd style=\u0027border: 1px solid #ccc; padding: 4px;\u0027\u003E~Cell~\u003C/td\u003E""
    }
  },
  {
    ""ObjectType"": 7,
    ""Body"": ""\u003Ch1\u003ESynonym: \u003Ccode\u003E~ObjectFullName~\u003C/code\u003E\u003C/h1\u003E\r\n\r\n\u003Cblockquote\u003E~Description~\u003C/blockquote\u003E\r\n\r\n\u003Ch3\u003EBase Object Name: ~BaseObjectName~\u003C/h3\u003E\r\n\u003Ch3\u003EBase Object Type: ~BaseObjectType~\u003C/h3\u003E\r\n""
  }
]";

        private const string wikiTemplate = @"[
  {
    ""ObjectType"": 0,
    ""Body"": ""= Table: ~ObjectFullName~ =\n\n'''Description:''' ~Description~\n\n'''Columns:'''\n~Columns~\n\n'''Indexes:'''\n~Indexes~\n\n'''Constraints:'''\n~Constraints~\n----"",
    ""Columns"": {
      ""Body"": ""{| class=\""wikitable\"" style=\""border: 1px solid #a2a9b1;\""\n! Ord\n! Name\n! Data Type\n! Description\n~ColumnItem~\n|}"",
      ""ColumnRow"": ""|-\n| ~ColumnOrd~ || <code>~ColumnName~</code> || ~ColumnDataType~ || ~ColumnDescription~""
    },
    ""Constraints"": {
      ""Body"": ""{| class=\""wikitable\"" style=\""border: 1px solid #a2a9b1;\""\n! Constraint Name\n! Type\n! Column\n~ConstraintItem~\n|}"",
      ""ConstraintRow"": ""|-\n| <code>~ConstraintName~</code> || ~ConstraintType~ || ~ConstraintColumn~""
    },
    ""Indexes"": {
      ""Body"": ""{| class=\""wikitable\"" style=\""border: 1px solid #a2a9b1;\""\n! Index Name\n! Type\n! Columns\n! Unique\n~IndexItem~\n|}"",
      ""IndexRow"": ""|-\n| <code>~IndexName~</code> || ~IndexType~ || ~IndexColumns~ || ~UniqueIndex~""
    }
  },
  {
    ""ObjectType"": 1,
    ""Body"": ""= View: ~ObjectFullName~ =\n\n'''Description:''' ~Description~\n\n'''Columns:'''\n~Columns~\n\n'''Indexes:'''\n~Indexes~\n\n'''Constraints:'''\n~Constraints~\n----"",
    ""Columns"": {
      ""Body"": ""{| class=\""wikitable\"" style=\""border: 1px solid #a2a9b1;\""\n! Ord\n! Name\n! Data Type\n! Description\n~ColumnItem~\n|}"",
      ""ColumnRow"": ""|-\n| ~ColumnOrd~ || <code>~ColumnName~</code> || ~ColumnDataType~ || ~ColumnDescription~""
    },
    ""Constraints"": {
      ""Body"": ""{| class=\""wikitable\"" style=\""border: 1px solid #a2a9b1;\""\n! Constraint Name\n! Type\n! Column\n~ConstraintItem~\n|}"",
      ""ConstraintRow"": ""|-\n| <code>~ConstraintName~</code> || ~ConstraintType~ || ~ConstraintColumn~""
    },
    ""Indexes"": {
      ""Body"": ""{| class=\""wikitable\"" style=\""border: 1px solid #a2a9b1;\""\n! Index Name\n! Type\n! Columns\n! Unique\n~IndexItem~\n|}"",
      ""IndexRow"": ""|-\n| <code>~IndexName~</code> || ~IndexType~ || ~IndexColumns~ || ~UniqueIndex~""
    }
  },
  {
    ""ObjectType"": 2,
    ""Body"": ""= Function: ~ObjectFullName~ =\n\n'''Description:''' ~Description~\n\n'''Parameters:'''\n~Parameters~\n\n'''SQL Code:'''\n<syntaxhighlight lang=\""sql\"">\n~Definition~\n</syntaxhighlight>\n"",
    ""Parameters"": {
      ""Body"": ""{| class=\""wikitable\""\n! Ord\n! Name\n! Data Type\n! Direction\n! Description\n~ParameterItem~\n|}"",
      ""ParameterRow"": ""|-\n| ~ParameterOrd~ || <code>~ParameterName~</code> || ~ParameterDataType~ || ~ParameterDirection~ || ~ParameterDescription~""
    }
  },
  {
    ""ObjectType"": 3,
    ""Body"": ""= Stored Procedure: ~ObjectFullName~ =\n\n'''Description:''' ~Description~\n\n'''Parameters:'''\n~Parameters~\n\n'''SQL Code:'''\n<syntaxhighlight lang=\""sql\"">\n~Definition~\n</syntaxhighlight>\n"",
    ""Parameters"": {
      ""Body"": ""{| class=\""wikitable\""\n! Ord\n! Name\n! Data Type\n! Direction\n! Description\n~ParameterItem~\n|}"",
      ""ParameterRow"": ""|-\n| ~ParameterOrd~ || <code>~ParameterName~</code> || ~ParameterDataType~ || ~ParameterDirection~ || ~ParameterDescription~""
    }
  },
  {
    ""ObjectType"": 4,
    ""Body"": ""= Trigger: ~ObjectFullName~ =\n\n'''Description:''' ~Description~\n\n'''Parameters:'''\n~Parameters~\n\n'''SQL Code:'''\n<syntaxhighlight lang=\""sql\"">\n~Definition~\n</syntaxhighlight>\n""
  },
  {
    ""ObjectType"": 5,
    ""Body"": ""{| class=\""wikitable\""\n! Schema\n! Name\n! Description\n~ObjectItem~\n|}"",
    ""ObjectLists"": {
      ""Body"": """",
      ""ObjectRow"": ""|-\n| ~ObjectSchema~ || <code>~ObjectName~</code> || ~Description~""
    }
  },
  {
    ""ObjectType"": 6,
    ""Body"": ""{| class=\""wikitable\""\n! ~Header~\n~Rows~\n|}"",
    ""DataTable"": {
      ""Body"": """",
      ""DataRow"": ""|-\n~Row~"",
      ""HeaderCell"": ""! ~HeaderCell~"",
      ""Cell"": ""| ~Cell~""
    }
  },
  {
    ""ObjectType"": 7,
    ""Body"": ""= Synonym: {{code|~ObjectFullName~}} =\r\n\r\n\u003E ~Description~\r\n\r\n=== Base Object Name: ~BaseObjectName~ ===\r\n=== Base Object Type: ~BaseObjectType~ ===\r\n""
  }
]";

        private readonly List<Template> _templates = [];

        /// <summary>
        /// Returns Template items
        /// </summary>
        public List<Template> TemplateLists
        {
            get
            {
                return _templates;
            }
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>A Template.</returns>
        public Template? GetTemplate(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return null; // Return null if documentType is null or empty
            }

            // Find the template based on documentType
            var template = _templates.FirstOrDefault(t => t.DocumentType.Equals(documentType, StringComparison.OrdinalIgnoreCase));
            if (template == null)
            {
                // If not found, create a new one
                template = new Template(documentType);
                _templates.Add(template);
            }
            return template;
        }

        /// <summary>
        /// Load Template items from the application data file
        /// </summary>
        public void Load()
        {
            var templatelist = new List<string> { "SharePoint", "Markdown", "WIKI" };

            // open the Templates.dat file
            string filePath = FilePath();
            if (File.Exists(filePath))
            {
                // check if the file is start with "<root>", if so, clear the file
                string firstLine = File.ReadLines(filePath).FirstOrDefault() ?? string.Empty;
                if (firstLine.StartsWith("<root>", StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllText(filePath, string.Empty); // Clear the file
                }

                // Load the templates from the file
                string[] lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !templatelist.Contains(line, StringComparer.OrdinalIgnoreCase))
                    {
                        templatelist.Add(line);
                    }
                }
            }

            // Initialize the templates list with default templates if needed
            if (_templates.Count == 0)
            {
                foreach (var templateName in templatelist)
                {
                    AddDocumentType(templateName);
                }
            }
        }

        /// <summary>
        /// Resets the template.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>A bool.</returns>
        public bool ResetTemplate(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return false; // Return false if documentType is null or empty
            }

            // Find the template based on documentType
            var template = _templates.FirstOrDefault(t => t.DocumentType.Equals(documentType, StringComparison.OrdinalIgnoreCase));
            if (template == null)
            {
                return false; // Template not found
            }

            // Reset the template
            string templateJson = documentType.ToUpper() switch
            {
                "SHAREPOINT" => sharePointTemplate,
                "MARKDOWN" => markdownTemplate,
                "WIKI" => wikiTemplate,
                _ => ""
            };

            if (!string.IsNullOrEmpty(templateJson))
            {
                template.Reset(templateJson);
                template.Save();
                return true; // Reset successful
            }
            
            return false; // No valid template found to reset
        }

        /// <summary>
        /// Adds the document type.
        /// </summary>
        /// <param name="newDocumentType">The new document type.</param>
        internal void AddDocumentType(string newDocumentType)
        {
            // Check if the document type already exists
            if (_templates.Any(t => t.DocumentType.Equals(newDocumentType, StringComparison.OrdinalIgnoreCase)))
            {
                return; // Document type already exists, no need to add
            }

            // Create a new Template object with the provided document type
            var template = new Template(newDocumentType);
            if (!template.Load())
            {
                string templateJson = newDocumentType.ToUpper() switch
                {
                    "SHAREPOINT" => sharePointTemplate,
                    "MARKDOWN" => markdownTemplate,
                    "WIKI" => wikiTemplate,
                    _ => ""
                };

                if (!string.IsNullOrEmpty(templateJson))
                {
                    template.Reset(templateJson);
                }
                else
                {
                    template.Initialize();
                }
                template.Save();
            }

            _templates.Add(template);

            // add the new document type to the file with FilePath()
            string filePath = FilePath();
            bool existsInFile = false;
            if (File.Exists(filePath))
            {
                // Read all lines and check if the document type already exists (case-insensitive)
                existsInFile = File.ReadLines(filePath)
                    .Any(line => line.Equals(newDocumentType, StringComparison.OrdinalIgnoreCase));
            }

            if (!existsInFile)
            {
                using StreamWriter sw = new(filePath, true);
                sw.WriteLine(newDocumentType); // Append the new document type to the file
            }
        }

        /// <summary>
        /// Saves the.
        /// </summary>
        internal void Save()
        {
            // save for each template in the list
            foreach (var template in _templates)
            {
                template.Save();
            }
        }

        /// <summary>
        /// Returns the local where to store the Templates data
        /// </summary>
        /// <returns></returns>
        private static string FilePath()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "SQLDocBuilder");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            return Path.Combine(dataPath, "Templates.dat");
        }
    }
}
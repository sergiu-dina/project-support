#pragma checksum "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Gantt\Gantt.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f26a0b4fb02d3b2780a37196ade67a444a0aeaa8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Gantt_Gantt), @"mvc.1.0.view", @"/Views/Gantt/Gantt.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\_ViewImports.cshtml"
using ProjectSupport;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\_ViewImports.cshtml"
using ProjectSupport.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f26a0b4fb02d3b2780a37196ade67a444a0aeaa8", @"/Views/Gantt/Gantt.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2adc4f4e8a83aa64425c23f44ddedb505eee7e37", @"/Views/_ViewImports.cshtml")]
    public class Views_Gantt_Gantt : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Gantt\Gantt.cshtml"
  
    ViewData["Title"] = "Home Page";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n<div id=\"ganttContainer\" style=\"width: 80%; height: 100%; z-index:100; position:absolute; top:100px; left:300px\"></div>\r\n\r\n\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral(@"
    <script>
        (function () {
            // add month scale
            gantt.config.scale_unit = ""month"";
            gantt.config.step = 1;
            gantt.templates.date_scale = function (date) {
                var dateToStr = gantt.date.date_to_str(""%d %M"");
                var endDate = gantt.date.add(gantt.date.add(date, 1, ""month""), -1, ""day"");
                return dateToStr(date) + "" - "" + dateToStr(endDate);
            };
            gantt.config.subscales = [
                { unit: ""day"", step: 1, date: ""%D"" }
            ];
            gantt.config.scale_height = 50;

            // configure milestone description
            gantt.templates.rightside_text = function (start, end, task) {
                if (task.type == gantt.config.types.milestone) {
                    return task.text;
                }
                return """";
            };
            // add section to type selection: task, project or milestone
            gantt.config.lightbox.section");
                WriteLiteral(@"s = [
                { name: ""description"", height: 70, map_to: ""text"", type: ""textarea"", focus: true },
                { name: ""type"", type: ""typeselect"", map_to: ""type"" },
                { name: ""time"", height: 72, type: ""duration"", map_to: ""auto"" }
            ];

            gantt.config.xml_date = ""%Y-%m-%d %H:%i:%s""; // format of dates in XML
            gantt.init(""ganttContainer""); // initialize gantt
        })();
    </script>
");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
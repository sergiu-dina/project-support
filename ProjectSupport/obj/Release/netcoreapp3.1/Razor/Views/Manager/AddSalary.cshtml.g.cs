#pragma checksum "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "68cef3e5da74983f9288041e72a5bd02fa0380ba"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Manager_AddSalary), @"mvc.1.0.view", @"/Views/Manager/AddSalary.cshtml")]
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
#nullable restore
#line 3 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
using ProjectSupport.Areas.Identity.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"68cef3e5da74983f9288041e72a5bd02fa0380ba", @"/Views/Manager/AddSalary.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2adc4f4e8a83aa64425c23f44ddedb505eee7e37", @"/Views/_ViewImports.cshtml")]
    public class Views_Manager_AddSalary : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ProjectSupport.ViewModels.EditUserViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("text-center"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/img/EditUser.svg"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "SeeDevelopers", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Manager", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            WriteLiteral("\r\n");
            WriteLiteral("\r\n");
#nullable restore
#line 9 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
  
    ViewBag.Title = "Edit User";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"container child\">\r\n    <div class=\"content-container\">\r\n        <div class=\"container-fluid\">\r\n\r\n            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "68cef3e5da74983f9288041e72a5bd02fa0380ba5816", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 19 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
             using (Html.BeginForm())
            {
                

#line default
#line hidden
#nullable disable
#nullable restore
#line 21 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
           Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <div class=""form-horizontal"" style=""padding-left:76px"">
                    <div class=""card"">
                        <div class=""card-header"">
                            <h4>Edit User</h4>
                        </div>
                        <div class=""card-body"">
                            ");
#nullable restore
#line 29 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.ValidationSummary(true, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 30 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 31 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.UserName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 32 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.NormalizedUserName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 33 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 34 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.NormalizedEmail));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 35 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.EmailConfirmed));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 36 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.PasswordHash));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 37 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.SecurityStamp));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 38 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.ConcurrencyStamp));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 39 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.PhoneNumber));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 40 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.PhoneNumberConfirmed));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 41 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.TwoFactorEnabled));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 42 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.LockoutEnd));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 43 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.LockoutEnabled));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 44 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.AccessFailedCount));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 45 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.FirstName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 46 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.LastName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 47 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.AppUser.ProfilePicture));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            ");
#nullable restore
#line 48 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                       Write(Html.HiddenFor(model => model.UserId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n                            <div class=\"form-group\">\r\n                                ");
#nullable restore
#line 51 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                           Write(Html.LabelFor(model => model.AppUser.HourlyRate, htmlAttributes: new { @class = "control-label col-md-2" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                <div class=\"col-md-10\">\r\n                                    ");
#nullable restore
#line 53 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                               Write(Html.EditorFor(model => model.AppUser.HourlyRate, new { htmlAttributes = new { @class = "form-control" } }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    ");
#nullable restore
#line 54 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                               Write(Html.ValidationMessageFor(model => model.AppUser.HourlyRate, "", new { @class = "text-danger" }));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                                </div>
                            </div>
                        </div>
                        <div class=""card-footer"">
                            <input type=""submit"" value=""Save"" class=""btn btn-primary"" />
                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "68cef3e5da74983f9288041e72a5bd02fa0380ba15475", async() => {
                WriteLiteral("\r\n                                Back to List\r\n                            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 61 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                                                                                     WriteLiteral(UserManager.GetUserId(User));

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 61 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
                                                                                                                                 WriteLiteral(ViewBag.Page);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["pg"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-pg", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["pg"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n");
#nullable restore
#line 67 "E:\Facultate\Licenta\project-support\ProjectSupport\Views\Manager\AddSalary.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </div>\r\n    </div>\r\n</div>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public UserManager<AppUser> UserManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public SignInManager<AppUser> SignInManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ProjectSupport.ViewModels.EditUserViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591

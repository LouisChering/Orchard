﻿using System;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Scripting.CSharp.Models;
using Orchard.Scripting.CSharp.Services;
using Orchard.Scripting.CSharp.Settings;

namespace Orchard.Scripting.CSharp.Drivers {
    [OrchardFeature("Orchard.Scripting.CSharp.Validation")]
    public class ScriptValidationPartDriver : ContentPartDriver<ScriptValidationPart> {
        private readonly ICSharpService _csharpService;
        private readonly IOrchardServices _orchardServices;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ScriptValidationPartDriver(
            ICSharpService csharpService, 
            IOrchardServices orchardServices, 
            IWorkContextAccessor workContextAccessor) {
            _csharpService = csharpService;
            _orchardServices = orchardServices;
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        protected override string Prefix {
            get { return "SpamFilter"; }
        }

        protected override DriverResult Editor(ScriptValidationPart part, Orchard.ContentManagement.IUpdateModel updater, dynamic shapeHelper) {
            var script = part.Settings.GetModel<ScriptValidationPartSettings>().Script;

            if (!String.IsNullOrWhiteSpace(script)) {

                _csharpService.SetParameter("orchardServices", _orchardServices);
                _csharpService.SetParameter("contentItem", (dynamic)part.ContentItem);
                _csharpService.SetParameter("workContext", _workContextAccessor.GetContext());
                _csharpService.SetFunction("t", (Func<string, string>)(x => T(x).Text));
                _csharpService.SetFunction("addModelError", (Action<string>)(x => updater.AddModelError("Script", T(x))));

                _csharpService.Run(script);
            }

            return Editor(part, shapeHelper);
        }
    }
}
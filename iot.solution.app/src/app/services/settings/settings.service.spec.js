"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var settings_service_1 = require("./settings.service");
describe('SettingsService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(settings_service_1.SettingsService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=settings.service.spec.js.map
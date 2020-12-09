"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var alerts_service_1 = require("./alerts.service");
describe('AlertsService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(alerts_service_1.AlertsService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=alerts.service.spec.js.map
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var dashboard_service_1 = require("./dashboard.service");
describe('DashboardService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(dashboard_service_1.DashboardService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=dashboard.service.spec.js.map
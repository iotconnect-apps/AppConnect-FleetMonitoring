"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var fleet_service_1 = require("./fleet.service");
describe('FleetService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(fleet_service_1.FleetService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=fleet.service.spec.js.map
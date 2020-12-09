"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var driver_service_1 = require("./driver.service");
describe('DriverService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(driver_service_1.DriverService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=driver.service.spec.js.map
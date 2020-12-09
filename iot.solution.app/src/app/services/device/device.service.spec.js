"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var device_service_1 = require("./device.service");
describe('DeviceService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(device_service_1.DeviceService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=device.service.spec.js.map
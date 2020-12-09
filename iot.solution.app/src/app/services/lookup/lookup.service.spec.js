"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var lookup_service_1 = require("./lookup.service");
describe('LookupService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(lookup_service_1.LookupService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=lookup.service.spec.js.map
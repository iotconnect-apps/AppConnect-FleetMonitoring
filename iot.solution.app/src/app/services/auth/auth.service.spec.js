"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var auth_service_1 = require("./auth.service");
describe('AuthService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(auth_service_1.AuthService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=auth.service.spec.js.map
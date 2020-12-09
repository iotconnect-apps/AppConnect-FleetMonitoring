"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var user_service_1 = require("./user.service");
describe('UserService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(user_service_1.UserService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=user.service.spec.js.map
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var roles_service_1 = require("./roles.service");
describe('RolesService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(roles_service_1.RolesService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=roles.service.spec.js.map
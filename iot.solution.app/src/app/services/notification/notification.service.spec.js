"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var notification_service_1 = require("./notification.service");
describe('NotificationService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(notification_service_1.NotificationService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=notification.service.spec.js.map
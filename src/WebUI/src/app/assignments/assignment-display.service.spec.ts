import { TestBed } from "@angular/core/testing";
import { AssignmentDisplayService } from "./assignment-display.service";
import { AuthService } from "../auth/auth.service";
import { DummyDataHelper } from "src/tests/helpers/dummy-data-helper";
import { Board } from "../models/board";
import { AssignmentDisplayModel } from "../models/display-models/assignment-display-model";
import * as moment from 'moment';

describe('AssignmentDisplayService', () => {
    let service: AssignmentDisplayService;
    let authMock: jasmine.SpyObj<AuthService>;

    beforeEach(() => {
      authMock = jasmine.createSpyObj('AuthService',
        ['isAdmin', 'isManager', 'isEmployee', 'getEmployeeId']);
      authMock.isAdmin.and.returnValue(false);
      authMock.isManager.and.returnValue(false);
      authMock.isEmployee.and.returnValue(false);
      authMock.getEmployeeId.and.returnValue(null);

      TestBed.configureTestingModule({
        providers: [
          AssignmentDisplayService,
          { provide: AuthService, useValue: authMock }
        ]
      })
      .compileComponents();

      service = TestBed.inject(AssignmentDisplayService);
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

    it('should get display models from board for all assignment if called by manager', () => {
      authMock.isManager.and.returnValue(true);
      const boards: Board[] = DummyDataHelper.getDummyBoards();

      const result: AssignmentDisplayModel[] = service.getAssignmentDisplayModels(boards);

      expect(result.length).toEqual(5);
    });

    it('should get display models from board for the employee\'s assignment', () => {
      authMock.isEmployee.and.returnValue(true);
      authMock.getEmployeeId.and.returnValue('1');
      const boards: Board[] = DummyDataHelper.getDummyBoards();

      const result: AssignmentDisplayModel[] = service.getAssignmentDisplayModels(boards);

      expect(result.length).toEqual(3);
    });

    it('should combine date (as moment) and time in deadline', () => {
      const date = moment(new Date(2023, 10, 14));
      const time = '15:25';
      const expected = new Date(2023, 10, 14, 17, 25)

      const deadline = service.getDeadline(date, time)

      expect(deadline).toEqual(expected);
    });

    it('should combine date (as string) and time in deadline', () => {
      const date = '2023-10-14T00:00:00'
      const time = '15:25';
      const expected = new Date(2023, 9, 14, 18, 25)

      const deadline = service.getDeadline(date, time)

      expect(deadline).toEqual(expected);
    });

    it('should get time from datetime', () => {
      const date = new Date(2023, 9, 14, 18, 25)
      const expected = "18:25"

      const time = service.getTimeFromDateTime(date)

      expect(time).toEqual(expected);
    });

    it('should return true to isUserAuthorizeToModifyTask when called by manager', () => {
      authMock.isManager.and.returnValue(true);
      const task = DummyDataHelper.getDummyAssignments()[0];

      const result = service.isUserAuthorizeToModifyTask(task);

      expect(result).toEqual(true);
    });

    it('should return true to isUserAuthorizeToModifyTask when the employee is responsible for the task', () => {
      authMock.isEmployee.and.returnValue(true);
      authMock.getEmployeeId.and.returnValue('1');
      const task = DummyDataHelper.getDummyAssignments()[0];

      const result = service.isUserAuthorizeToModifyTask(task);

      expect(result).toEqual(true);
    });

    it('should return false to isUserAuthorizeToModifyTask when the employee is not responsible for the task', () => {
      authMock.isEmployee.and.returnValue(true);
      authMock.getEmployeeId.and.returnValue('2');
      const task = DummyDataHelper.getDummyAssignments()[0];

      const result = service.isUserAuthorizeToModifyTask(task);

      expect(result).toEqual(false);
    });
});
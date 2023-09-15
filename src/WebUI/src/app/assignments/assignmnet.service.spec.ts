import { TestBed } from '@angular/core/testing';
import { AssignmentService } from './assignment.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DummyDataHelper } from 'src/tests/helpers/dummy-data-helper';
import { environment } from 'src/environments/environment';
import { Assignment } from '../models/assignment';

describe('AssignmentService', () => {
    let service: AssignmentService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule
        ],
        providers: [
          AssignmentService
        ]
      })
      .compileComponents();

      service = TestBed.inject(AssignmentService);
      httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

    it('getAssignments should call GET api/boards/:boardId/tasks/', () => {
      const boardId: number = 1;
      service.getAssignments(boardId).subscribe();

      const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/');
      expect(req.request.method).toBe('GET');
    });

    it('getAssignment should call GET api/boards/:boardId/tasks/:taskId/', () => {
      const boardId: number = 1;
      const taskId: number = 1;
      service.getAssignment(boardId, taskId).subscribe();

      const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId);
      expect(req.request.method).toBe('GET');
    });

   it('createAssignment should call POST api/boards/:boardId/tasks/', () => {
    const boardId: number = 1;
    const toCreate: Assignment = DummyDataHelper.getDummyAssignments()[0];
    service.createAssignment(boardId, toCreate).subscribe();

    const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/');
    expect(req.request.method).toBe('POST');
   });

   it('updateAssignment should call PUT api/boards/:boardId/tasks/:taskId/', () => {
    const boardId: number = 1;
    const taskId: number = 1;
    const toUpdate: Assignment = DummyDataHelper.getDummyAssignments()[0];

    service.updateAssignment(boardId, toUpdate).subscribe();

    const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId);
    expect(req.request.method).toBe('PUT');
   });

   it('moveAssignmentToTheStage should call PUT api/boards/:boardId/tasks/:taskId/move/:stageId/', () => {
    const boardId: number = 1;
    const taskId: number = 1;
    const stageId: number = 2;

    service.moveAssignmentToTheStage(boardId, taskId, stageId).subscribe();

    const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId + '/move/' + stageId);
    expect(req.request.method).toBe('PUT');
   });

   it('changeAssignmentStatus should call PUT api/boards/:boardId/tasks/:taskId/complete/', () => {
    const boardId: number = 1;
    const taskId: number = 1;

    service.changeAssignmentStatus(boardId, taskId, true).subscribe();

    const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId + '/complete');
    expect(req.request.method).toBe('PUT');
   });

   it('changeAssignmentStatus should call PUT api/boards/:boardId/tasks/:taskId/uncomplete/', () => {
    const boardId: number = 1;
    const taskId: number = 1;

    service.changeAssignmentStatus(boardId, taskId, false).subscribe();

    const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId + '/uncomplete');
    expect(req.request.method).toBe('PUT');
   });

   it('deleteAssignment should call DELETE api/boards/:boardId/tasks/:taskId/', () => {
    const boardId: number = 1;
    const taskId: number = 1;

    service.deleteAssignment(boardId, taskId).subscribe();

    const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId);
    expect(req.request.method).toBe('DELETE');
   });
});
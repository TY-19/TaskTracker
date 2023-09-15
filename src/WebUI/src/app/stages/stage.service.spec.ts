import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { StageService } from "./stage.service";
import { environment } from "src/environments/environment";
import { DummyDataHelper } from "src/tests/helpers/dummy-data-helper";
import { Stage } from "../models/stage";

describe('StageService', () => {
    let service: StageService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule
        ],
        providers: [
          StageService
        ]
      })
      .compileComponents();

      service = TestBed.inject(StageService);
      httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

		it('getStages should call GET api/boards/:boardId/stages', () => {
      const boardId = 1;
			service.getStages(boardId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/stages');
      expect(req.request.method).toBe('GET');
    });

		it('getStage should call GET api/boards/:boardId/stages/:stageId', () => {
      const boardId = 1;
			const stageId = 1;
			service.getStage(boardId, stageId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/stages/' + stageId);
      expect(req.request.method).toBe('GET');
    });

		it('createStage should call POST api/boards/:boardId/stages', () => {
      const boardId = 1;
			const stage: Stage = DummyDataHelper.getDummyStages()[0];
			service.createStage(boardId, stage).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/stages');
      expect(req.request.method).toBe('POST');
    });

		it('updateStage should call PUT api/boards/:boardId/stages/:stageId', () => {
      const boardId = 1;
			const stageId = 1;
			const stage: Stage = DummyDataHelper.getDummyStages()[0];
			service.updateStage(boardId, stageId, stage).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/stages/' + stageId);
      expect(req.request.method).toBe('PUT');
    });

		it('moveStage should call PUT api/boards/:boardId/stages/:stageId/moveforward', () => {
      const boardId = 1;
			const stageId = 1;
			service.moveStage(boardId, stageId, true).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/stages/' + stageId + "/moveforward");
      expect(req.request.method).toBe('PUT');
    });

		it('moveStage should call PUT api/boards/:boardId/stages/:stageId/moveback', () => {
      const boardId = 1;
			const stageId = 1;
			service.moveStage(boardId, stageId, false).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/stages/' + stageId + "/moveback");
      expect(req.request.method).toBe('PUT');
    });

		it('deleteStage should call DELETE api/boards/:boardId/stages/:stageId', () => {
      const boardId = 1;
			const stageId = 1;
			service.deleteStage(boardId, stageId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/stages/' + stageId);
      expect(req.request.method).toBe('DELETE');
    });
});
import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { BoardService } from "./board.service";
import { environment } from "src/environments/environment";
import { DummyDataHelper } from "src/tests/helpers/dummy-data-helper";

describe('BoardService', () => {
    let service: BoardService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule
        ],
        providers: [
          BoardService
        ]
      })
      .compileComponents();

      service = TestBed.inject(BoardService);
      httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

		it('getBoards should call GET api/boards', () => {
			service.getBoards().subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards');
      expect(req.request.method).toBe('GET');
    });

		it('getBoardsOfTheEmployee should call GET api/boards/accessible', () => {
      service.getBoardsOfTheEmployee().subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/accessible');
      expect(req.request.method).toBe('GET');
    });

		it('getBoard should call GET api/boards/:boardId', () => {
			const boardId = 1;
      service.getBoard(boardId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId);
      expect(req.request.method).toBe('GET');
    });

		it('createBoard should call POST api/boards', () => {
      const board = DummyDataHelper.getDummyBoards()[0];
			service.createBoard(board).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards');
      expect(req.request.method).toBe('POST');
    });

		it('updateBoard should call PUT api/boards/:boardId', () => {
			const boardId = 1;
      const board = DummyDataHelper.getDummyBoards()[0];
			service.updateBoard(boardId, board).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId);
      expect(req.request.method).toBe('PUT');
    });

		it('deleteBoard should call DELETE api/boards/:boardId', () => {
      const boardId = 1;
			service.deleteBoard(boardId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId);
      expect(req.request.method).toBe('DELETE');
    });
});
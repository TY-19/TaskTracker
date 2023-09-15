import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Board } from "../models/board";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root',
  })
export class BoardService {
    
  constructor(private http: HttpClient) {

  }

  getBoards(): Observable<Board[]> {
      const url = environment.baseUrl + "api/boards";
      return this.http.get<Board[]>(url);
  }

  getBoardsOfTheEmployee(): Observable<Board[]> {
    const url = environment.baseUrl + "api/boards/accessible";
    return this.http.get<Board[]>(url);
}

  getBoard(id: number | string): Observable<Board> {
      const url = environment.baseUrl + "api/boards/" + id;
      return this.http.get<Board>(url);
  }

  createBoard(board: Board): Observable<Object> {
    const url = environment.baseUrl + "api/boards";
    return this.http.post(url, board);
  }

  updateBoard(id: number | string, board: Board): Observable<Object> {
      const url = environment.baseUrl + "api/boards/" + id;
      return this.http.put(url, board, { observe: 'response' });
  }

  deleteBoard(id: number | string): Observable<Object> {
    const url = environment.baseUrl + "api/boards/" + id;
    return this.http.delete(url);
  }
}
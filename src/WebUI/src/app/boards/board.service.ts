import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Board } from "../models/board";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root',
  })
export class BoardService {
    
  constructor(private http: HttpClient) {

  }

  getBoards() : Observable<Board[]> {
      const url = "/api/boards";
      return this.http.get<Board[]>(url);
  }

  getBoard(id: string) : Observable<Board> {
      const url = "/api/boards/" + id;
      return this.http.get<Board>(url);
  }

  createBoard(board: Board) {
    const url = "/api/boards/";
    return this.http.post(url, board);
  }

  updateBoard(id: string, board: Board) {
      const url = "/api/boards/" + id;
      return this.http.put(url, board, { observe: 'response' });
  }

  deleteBoard(id: string) {
    const url = "/api/boards/" + id;
    return this.http.delete(url);
  }
}
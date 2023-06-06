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

}
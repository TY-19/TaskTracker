import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Board } from "../models/board";
import { Observable } from "rxjs";
import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

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

    updateBoard(id: string, board: Board) {
        const url = "/api/boards/" + id;
        return this.http.put(url, board, { observe: 'response' });
    }

    boardNameValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
          const forbidden = !Number.isNaN(control.value) && 
            Number.parseInt(control.value) == control.value;
          return forbidden ? { nameIsNumber: {value: control.value}} : null;
        };
      }
}
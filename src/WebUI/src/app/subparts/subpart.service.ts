import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subpart } from "../models/subpart";

@Injectable({
    providedIn: 'root',
  })
export class SubpartService {
    constructor(private http: HttpClient) {

    }

    updateSubparts(boardId: string, assignmentId: number, subparts: Subpart[]) {
        const url = "/api/" + boardId + "/tasks/" + assignmentId + "/subparts";
        return this.http.put(url, subparts);
    }
}
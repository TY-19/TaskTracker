import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Assignment } from "../models/assignment";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root',
  })
export class AssignmentService {
    
  constructor(private http: HttpClient) {

  }

  getAssignment(boardId: string, assignmentId: string) : Observable<Assignment> {
    const url = "/api/boards/" + boardId + "/tasks/" + assignmentId;
    return this.http.get<Assignment>(url);
  }

  createAssignment(boardId: string, assignment: Assignment) {
    const url = "/api/boards/" + boardId + "/tasks/";
    return this.http.post(url, assignment);
  }

  updateAssignment(boardId: string, assignment: Assignment) {
    const url = "/api/boards/" + boardId + "/tasks/" + assignment.id;
    return this.http.put(url, assignment);
  }

  deleteAssignment(boardId: string, assignmentId: string) {
    const url = "/api/boards/" + boardId + "/tasks/" + assignmentId;
    return this.http.delete(url);
  }
}
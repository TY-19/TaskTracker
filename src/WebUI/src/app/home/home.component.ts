import { Component } from '@angular/core';
import { AssignmentsModes } from '../common/enums/assignments-modes';

@Component({
  selector: 'tt-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  readonly assignmentsMode: AssignmentsModes = AssignmentsModes.MultyBoards;
}

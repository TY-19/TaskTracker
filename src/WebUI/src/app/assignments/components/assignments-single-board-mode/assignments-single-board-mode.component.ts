import { Component, Input } from '@angular/core';

@Component({
  selector: 'tt-assignments-single-board-mode',
  templateUrl: './assignments-single-board-mode.component.html',
  styleUrls: ['./assignments-single-board-mode.component.scss']
})
export class AssignmentsSingleBoardModeComponent {
  @Input() boardId! : number | string;
}

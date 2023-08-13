import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/auth/auth.service';
import { Board } from 'src/app/models/board';

@Component({
  selector: 'tt-assignments-multy-boards-mode',
  templateUrl: './assignments-multy-boards-mode.component.html',
  styleUrls: ['./assignments-multy-boards-mode.component.scss']
})
export class AssignmentsMultyBoardsModeComponent {
  @Input() boards!: Board[];
  showCreateMenu: boolean = false;

  constructor(public authService: AuthService,
    private router: Router) { 
  }

  createAssignment(boardId: number) {
    this.router.navigate(['/boards', boardId, 'tasks', 'create']);
  }
}

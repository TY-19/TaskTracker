import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BoardService } from '../board.service';
import { UntypedFormControl, UntypedFormGroup,  Validators } from '@angular/forms';
import { Board } from 'src/app/models/board';
import { CustomValidators } from 'src/app/common/custom-validators';

@Component({
  selector: 'tt-board-edit',
  templateUrl: './board-edit.component.html',
  styleUrls: ['./board-edit.component.scss']
})
export class BoardEditComponent implements OnInit {

  board!: Board;
  form!: UntypedFormGroup;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private boardService: BoardService) { 

    }
  
  ngOnInit(): void {
    this.initiateForm();
    this.loadBoard();
  }

  private initiateForm(): void
  {
    this.form = new UntypedFormGroup({
      id: new UntypedFormControl(),
      name: new UntypedFormControl("", [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        CustomValidators.boardNameValidator()
      ])
    });
  }

  private loadBoard(): void {
    const idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.boardService.getBoard(idParam!)
      .subscribe(result => {
        this.board = result;
        this.form.patchValue(this.board);
      });
  }

  onSubmit(): void {
    if(this.form.valid)
    {
      this.board.name = this.form.controls['name'].value;
      this.updateBoard(this.board);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private updateBoard(board: Board): void {
    const idParam = this.activatedRoute.snapshot.paramMap.get('id');
    if (idParam && idParam === board.id.toString())
    {
      this.boardService.updateBoard(idParam, board)
        .subscribe(() => { this.router.navigate(['/boards']) });
    }
  }
}

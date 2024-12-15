import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForloopExampleComponent } from './forloop-example.component';

describe('ForloopExampleComponent', () => {
  let component: ForloopExampleComponent;
  let fixture: ComponentFixture<ForloopExampleComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ForloopExampleComponent]
    });
    fixture = TestBed.createComponent(ForloopExampleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

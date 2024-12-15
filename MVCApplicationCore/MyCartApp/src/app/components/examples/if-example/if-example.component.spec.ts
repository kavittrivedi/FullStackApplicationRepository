import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IfExampleComponent } from './if-example.component';

describe('IfExampleComponent', () => {
  let component: IfExampleComponent;
  let fixture: ComponentFixture<IfExampleComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [IfExampleComponent]
    });
    fixture = TestBed.createComponent(IfExampleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

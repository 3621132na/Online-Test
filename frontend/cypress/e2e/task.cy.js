describe('Task Management', () => {
  const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdDEyMyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiYTBmYzVlY2EtY2NiMS00MTk0LTlkNDItYTM4MjVjODBlZGUzIiwiZXhwIjoxNzQ2NjU0NzU1LCJpc3MiOiJTUE1TIiwiYXVkIjoiU1BNUyJ9.V_N1hH8Of8mE3GIRFM5NBt_xdKxtHPS3fw42GsKqROA';  // Đảm bảo token hợp lệ
  const taskData = {
    title: 'Test Task',
    description: 'This is a test task',
    status: 'ToDo',
    dueDate: '2025-05-08',
  };

  beforeEach(() => {
    // Lưu token vào localStorage để giả lập tình huống đã đăng nhập
    localStorage.setItem('token', token);

    // Truy cập vào trang chính sau khi đã đăng nhập
    cy.visit('/');

    // Kiểm tra xem có phải đã vào trang chính hay chưa
    cy.url().should('not.include', '/login');  // Đảm bảo không bị chuyển đến /login
  });

  it('should create, update, and delete a task', () => {
    // Đảm bảo form đã render
    cy.get('input[name="title"]').should('be.visible');
    cy.get('textarea[name="description"]').should('be.visible');
    cy.get('select[name="status"]').should('be.visible');
    cy.get('input[name="dueDate"]').should('be.visible');
    cy.contains('Add Task').should('be.visible');
    // Tạo task mới
    cy.get('input[name="title"]').type(taskData.title);  // Gõ vào title
    cy.get('textarea[name="description"]').type(taskData.description);
    cy.get('select[name="status"]').select(taskData.status);
    cy.get('input[name="dueDate"]').type(taskData.dueDate);

    // Kiểm tra task mới đã được thêm vào danh sách
    cy.contains(taskData.title).should('be.visible');

    // Cập nhật task
    cy.contains(taskData.title).parent().find('button').contains('Edit').click();
    cy.get('input[name="title"]').clear().type('Updated Test Task');
    cy.contains('Update Task').click();

    // Kiểm tra task đã được cập nhật
    cy.contains('Updated Test Task').should('be.visible');

    // Xóa task
    cy.contains('Updated Test Task').parent().find('button').contains('Delete').click();

    // Kiểm tra task đã bị xóa
    cy.contains('Updated Test Task').should('not.exist');
  });

  it('should filter tasks by status', () => {
    // Đảm bảo filter status có sẵn
    cy.get('select[name="filterStatus"]').should('be.visible');
    cy.get('select[name="filterStatus"]').select('Completed');
    cy.contains('Search').click();

    // Kiểm tra task có trạng thái "Completed"
    cy.get('.grid.gap-4').children().each((task) => {
      cy.wrap(task).contains('Completed').should('be.visible');
    });
  });

  it('should sort tasks by due date', () => {
    // Đảm bảo sort order có sẵn
    cy.get('select[name="sortOrder"]').should('be.visible');
    cy.get('select[name="sortOrder"]').select('asc');
    cy.contains('Search').click();

    // Kiểm tra sắp xếp theo ngày đến hạn
    let previousDate = null;
    cy.get('.grid.gap-4').children().each((task) => {
      const taskDueDate = new Date(
          task.find('p').last().text().replace('Due: ', '')
      ).getTime();
      if (previousDate) {
        expect(taskDueDate).to.be.greaterThan(previousDate);
      }
      previousDate = taskDueDate;
    });
  });

  it('should search tasks by keyword', () => {
    const keyword = 'Test';
    // Đảm bảo ô tìm kiếm có sẵn
    cy.get('input[name="keyword"]').should('be.visible');
    cy.get('input[name="keyword"]').type(keyword);
    cy.contains('Search').click();

    // Kiểm tra tìm kiếm theo từ khóa
    cy.get('.grid.gap-4').children().each((task) => {
      cy.wrap(task).contains(keyword).should('be.visible');
    });
  });

  it('should log out user and redirect to login', () => {
    // Đảm bảo nút logout có sẵn
    cy.contains('Logout').should('be.visible').click();
    cy.url().should('include', '/login');
  });
});

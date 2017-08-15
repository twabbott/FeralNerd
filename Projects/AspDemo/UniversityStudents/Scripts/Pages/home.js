(function (angular, model) {
    var pageModule = angular.module('app', []);
    pageModule.controller('appCtrl', ['$scope', '$http', _pageController]);

    //*** Internal functions ******************************
    function _validateControls($scope) {
        var validated = true;

        // First name textbox
        if (!$scope.form.firstNameEdit) {
            $scope.validation.firstNameError = "First name not given.";
            validated = false;
        }
        else
            $scope.validation.firstNameError = null;

        // Last name textbox
        if (!$scope.form.lastNameEdit) {
            $scope.validation.lastNameError = "Last name not given.";
            validated = false;
        }
        else
            $scope.validation.lastNameError = null;

        // Gender radio buttons
        if ($scope.form.genderRadio != "male" && $scope.form.genderRadio != "female") {
            $scope.validation.genderError = "Gender not given.";
            validated = false;
        }
        else
            $scope.validation.genderError = null;

        // Major textbox
        if (!$scope.form.majorEdit) {
            $scope.validation.majorError = "Major not given.";
            validated = false;
        }
        else
            $scope.validation.majorError = null;

        // GPA textbox
        if (!$scope.form.gpaEdit) {
            $scope.validation.gpaError = "GPA not given.";
            validated = false;
        }
        else if (isNaN(parseFloat($scope.form.gpaEdit))) {
            $scope.validation.gpaError = "GPA must be a number.";
            validated = false;
        }
        else if (parseInt($scope.form.gpaEdit) < 0 || parseInt($scope.form.gpaEdit) > 4) {
            $scope.validation.gpaError = "GPA must be between zero and 4.0.";
            validated = false;
        }
        else
            $scope.validation.gpaError = null;

        // Return the final result
        return validated;
    };

    function _clearInputForm($scope) {
        $scope.form.title = "";
        $scope.form.object = null;
        $scope.form.firstNameEdit = "";
        $scope.form.lastNameEdit = "";
        $scope.form.genderRadio = "";
        $scope.form.majorEdit = "";
        $scope.form.gpaEdit = "";
        $scope.form.createNew = false;

        $scope.validation.firstNameError = null;
        $scope.validation.lastNameError = null;
        $scope.validation.genderError = null;
        $scope.validation.majorError = null;
        $scope.validation.gpaError = null;
    };

    function _pageController($scope, $http) {
        // These are used to hold form / validation data
        $scope.form = {};
        $scope.validation = {};
        $scope.searchText = "";
        $scope.studentList = [];

        _clearInputForm($scope);

        $scope.ShowAllBtnClicked = function () {
            waitingDialog.show();

            $http.get("/api/students")
            .then(function (response) {
                $scope.studentList = response.data;

                waitingDialog.hide();
            })
        };

        $scope.SearchBtnClicked = function () {
            if ($scope.searchText === '') {
                $scope.studentList = [];
                return;
            }

            waitingDialog.show();

            $http.get("/api/students?search=" + $scope.searchText)
            .then(function (response) {
                $scope.studentList = response.data;

                waitingDialog.hide();
            })
        };

        $scope.CreateBtnClicked = function () {
            _clearInputForm($scope);
            $scope.form.title = "Create New Student";
            $scope.form.createNew = true;
        }

        $scope.CreateDlgBtnClicked = function () {
            if (!_validateControls($scope))
                return false;

            // Have to use JQuery here.  AngularJS knows nothing about Bootstrap
            $('#studentModal').modal('hide');
            waitingDialog.show();

            var student = {
                FirstName: $scope.form.firstNameEdit,
                LastName: $scope.form.lastNameEdit,
                Gender: $scope.form.genderRadio == "male"? 0: 1,
                Major: $scope.form.majorEdit,
                Gpa: parseFloat($scope.form.gpaEdit)
            }

            $http.post("/api/students/", student)
            .then(function (response) {
                $scope.studentList.push(response.data);

                waitingDialog.hide();
            })
        }

        $scope.EditBtnClicked = function (student) {
            _clearInputForm($scope);
            $scope.form.title = "Edit Student";
            $scope.form.object = student;
            $scope.form.firstNameEdit = student.FirstName;
            $scope.form.lastNameEdit = student.LastName;
            $scope.form.genderRadio = student.Gender === 0? "male": "female";
            $scope.form.majorEdit = student.Major;
            $scope.form.gpaEdit = student.Gpa;
            $scope.form.createNew = false;

            $('#studentModal').modal('show');
        }

        $scope.SaveDlgBtnClicked = function () {
            if (!_validateControls($scope))
                return false;

            // Have to use JQuery here.  AngularJS knows nothing about Bootstrap
            $('#studentModal').modal('hide');
            waitingDialog.show();

            var student = {
                FirstName: $scope.form.firstNameEdit,
                LastName: $scope.form.lastNameEdit,
                Gender: $scope.form.genderRadio == "male" ? 0 : 1,
                Major: $scope.form.majorEdit,
                Gpa: parseFloat($scope.form.gpaEdit)
            }

            $http.put("/api/students/" + $scope.form.object.StudentId, student)
            .then(function (response) {
                $scope.form.object.FirstName = $scope.form.firstNameEdit,
                $scope.form.object.LastName = $scope.form.lastNameEdit,
                $scope.form.object.Gender = $scope.form.genderRadio == "male" ? 0 : 1,
                $scope.form.object.Major = $scope.form.majorEdit,
                $scope.form.object.Gpa = parseFloat($scope.form.gpaEdit)

                waitingDialog.hide();
            })
        }

        $scope.DeleteBtnClicked = function (student) {
            $scope.form.object = student;
            $('#confirmDelete').modal('show');
        }

        $scope.ConfirmDeleteBtnClicked = function () {
            $('#confirmDelete').modal('hide');
            waitingDialog.show();

            $http.delete("/api/students/" + $scope.form.object.StudentId)
            .then(function (response) {
                for (var i = 0; i < $scope.studentList.length; i++) {
                    if ($scope.studentList[i].StudentId == $scope.form.object.StudentId) {
                        $scope.studentList.splice(i, 1);
                        break;
                    }
                }

                waitingDialog.hide();
            })
        }
    };
})(angular);


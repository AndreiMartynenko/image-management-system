package uk.ac.kingston.ci7250.ims.web.dto;

import jakarta.validation.constraints.NotBlank;

public class CreateStaffRequest {
    @NotBlank
    private String name;

    @NotBlank
    private String role;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getRole() {
        return role;
    }

    public void setRole(String role) {
        this.role = role;
    }
}

package uk.ac.kingston.ci7250.ims.web.dto;

import jakarta.validation.constraints.NotBlank;

public class CreatePatientRequest {
    @NotBlank
    private String name;

    private String address;
    private String conditions;
    private String diagnosis;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getConditions() {
        return conditions;
    }

    public void setConditions(String conditions) {
        this.conditions = conditions;
    }

    public String getDiagnosis() {
        return diagnosis;
    }

    public void setDiagnosis(String diagnosis) {
        this.diagnosis = diagnosis;
    }
}

package uk.ac.kingston.ci7250.ims.domain;

import java.util.ArrayList;
import java.util.List;

public class Patient {
    private final long id;
    private String name;
    private String address;
    private String conditions;
    private String diagnosis;
    private final List<Long> taskIds = new ArrayList<>();

    public Patient(long id, String name, String address, String conditions, String diagnosis) {
        this.id = id;
        this.name = name;
        this.address = address;
        this.conditions = conditions;
        this.diagnosis = diagnosis;
    }

    public long getId() {
        return id;
    }

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

    public List<Long> getTaskIds() {
        return taskIds;
    }
}

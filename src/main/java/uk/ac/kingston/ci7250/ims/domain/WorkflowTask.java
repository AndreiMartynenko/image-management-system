package uk.ac.kingston.ci7250.ims.domain;

import java.math.BigDecimal;
import java.time.Instant;

public class WorkflowTask {
    private final long id;
    private final long patientId;
    private final String type;
    private final Instant timestamp;
    private final BigDecimal cost;
    private final String performedBy;

    public WorkflowTask(long id, long patientId, String type, Instant timestamp, BigDecimal cost, String performedBy) {
        this.id = id;
        this.patientId = patientId;
        this.type = type;
        this.timestamp = timestamp;
        this.cost = cost;
        this.performedBy = performedBy;
    }

    public long getId() {
        return id;
    }

    public long getPatientId() {
        return patientId;
    }

    public String getType() {
        return type;
    }

    public Instant getTimestamp() {
        return timestamp;
    }

    public BigDecimal getCost() {
        return cost;
    }

    public String getPerformedBy() {
        return performedBy;
    }
}

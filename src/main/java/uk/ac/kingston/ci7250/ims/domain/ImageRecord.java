package uk.ac.kingston.ci7250.ims.domain;

import java.time.Instant;

public class ImageRecord {
    private final long id;
    private final long patientId;
    private final Modality modality;
    private final String diseaseTag;
    private final Instant uploadedAt;

    public ImageRecord(long id, long patientId, Modality modality, String diseaseTag, Instant uploadedAt) {
        this.id = id;
        this.patientId = patientId;
        this.modality = modality;
        this.diseaseTag = diseaseTag;
        this.uploadedAt = uploadedAt;
    }

    public long getId() {
        return id;
    }

    public long getPatientId() {
        return patientId;
    }

    public Modality getModality() {
        return modality;
    }

    public String getDiseaseTag() {
        return diseaseTag;
    }

    public Instant getUploadedAt() {
        return uploadedAt;
    }
}
